'''
Adapted on May 2024

@author: Ali Momenzadeh kholejani & Pawel Staszynski 


References: 
    Lab 5 Solution
'''
from rdflib import Graph
from rdflib import Namespace
import pandas as pd


class PartTwoSolution(object):

    def __init__(self, input_file):

        self.file = input_file

        #Dictionary that keeps the URIs. Specially useful if accessing a remote service to get a candidate URI to avoid repeated calls
        self.stringToURI = dict()

        #1. GRAPH INITIALIZATION

        #Empty graph
        self.g = Graph()

        #Note that this is the same namespace used in the ontology "pizza-restaurants-ontology.ttl"
        self.restaurants_ns_str = "http://www.semanticweb.org/city/in3067-inm713/2024/restaurants#"

        #Special namspaces class to create directly URIRefs in python.           
        self.restaurants = Namespace(self.restaurants_ns_str)

        #Prefixes for the serialization
        self.g.bind("restaurants#", self.restaurants) #restaurants is a newly created namespace

        #Load data in dataframe  
        self.data_frame = pd.read_csv(self.file, sep=',', quotechar='"',escapechar="\\")

    def performTask1(self):
        self.CovertCSVToRDF(False)
        
    def performTask2(self):
        self.CovertCSVToRDF(True)


    def CovertCSVToRDF(self, useExternalURI):
        for index, row in self.data_frame.iterrows():
            restaurant_uri = 
            



    def processLexicalName(self, name):
        #Remove potential spaces and other characters not allowed in URIs
        return name.replace(" ", "_").replace("(", "").replace(")", "") 

          
    def createURIForEntity(self, name, useExternalURI):
        
        #We create fresh URI (default option)
        self.stringToURI[name] = self.lab5_ns_str + self.processLexicalName(name)
        
        if useExternalURI: #We connect to online KG
            uri = self.getExternalKGURI(name)
            if uri!="":
                self.stringToURI[name]=uri
        
        return self.stringToURI[name]
    
    
        
    def getExternalKGURI(self, name):
        '''
        Approximate solution: We get the entity with highest lexical similarity
        The use of context may be necessary in some cases        
        '''
        
        entities = self.dbpedia.getKGEntities(name, 5)
        #print("Entities from DBPedia:")
        current_sim = -1
        current_uri=''
        for ent in entities:           
            isub_score = isub(name, ent.label) 
            if current_sim < isub_score:
                current_uri = ent.ident
                current_sim = isub_score
        
            #print(current_uri)
        return current_uri 
            
    
    '''
    Mapping to create triples like lab5:London rdf:type lab5:City
    A mapping may create more than one triple
    column: columns where the entity information is stored
    useExternalURI: if URI is fresh or from external KG
    '''
    def mappingToCreateTypeTriple(self, subject_column, class_type, useExternalURI):
        
        for subject in self.data_frame[subject_column]:
                
            #We use the ascii name to create the fresh URI for a city in the dataset
            if subject.lower() in self.stringToURI:
                entity_uri=self.stringToURI[subject.lower()]
            else:
                entity_uri=self.createURIForEntity(subject.lower(), useExternalURI)
            
            #TYPE TRIPLE
            #For the individuals we use URIRef to create an object "URI" out of the string URIs
            #For the concepts we use the ones in the ontology and we are using the NameSpace class
            #Alternatively one could use URIRef(self.lab5_ns_str+"City") for example 
            self.g.add((URIRef(entity_uri), RDF.type, class_type))
        


    def is_nan(self, x):
        return (x != x)  
            
    '''
    Mappings to create triples of the form lab5:london lab5:name "London"
    '''    
    def mappingToCreateLiteralTriple(self, subject_column, object_column, predicate, datatype):
        
        for subject, lit_value in zip(self.data_frame[subject_column], self.data_frame[object_column]):
            
            if self.is_nan(lit_value) or lit_value==None or lit_value=="":
                pass
            
            else:
                #Uri as already created
                entity_uri=self.stringToURI[subject.lower()]
                    
                #Literal
                lit = Literal(lit_value, datatype=datatype)
                
                #New triple
                self.g.add((URIRef(entity_uri), predicate, lit))
            
    '''
    Mappings to create triples of the form lab5:london lab5:cityIsLocatedIn lab5:united_kingdom
    '''
    def mappingToCreateObjectTriple(self, subject_column, object_column, predicate):
        
        for subject, object in zip(self.data_frame[subject_column], self.data_frame[object_column]):
            
            if self.is_nan(object):
                pass
            
            else:
                #Uri as already created
                subject_uri=self.stringToURI[subject.lower()]
                object_uri=self.stringToURI[object.lower()]
                    
                #New triple
                self.g.add((URIRef(subject_uri), predicate, URIRef(object_uri)))
            
    
    
    def mappingToCreateCapitalTriple(self, subject_column, object_column, capital_value_column):
        
        for subject, object, value in zip(self.data_frame[subject_column], self.data_frame[object_column], self.data_frame[capital_value_column]):
            
            #URI as already created
            subject_uri=self.stringToURI[subject.lower()]
            object_uri=self.stringToURI[object.lower()]
            
            
            #(default) if value is empty or not expected
            predicate = self.lab5.cityIsLocatedIn
            
            if value=="admin":                      
                predicate = self.lab5.isFirstLevelAdminCapitalOf
            elif value=="primary":
                predicate = self.lab5.isCapitalOf                        
            elif value=="minor":
                predicate = self.lab5.isSecondLevelAdminCapitalOf
            
            
            #New triple
            #Note that the ontology in lab5.ttl contains a hierarchy of properties, range and domain axioms and inverses
            #Via reasoning this triple will lead to several entailments
            self.g.add((URIRef(subject_uri), predicate, URIRef(object_uri)))
    





if __name__ == '__main__':

    #Format:
    #name  |  address  |  city  |  country  |  postcode  |  state  |  categories  |  menu item  |  item value  |  currency  |  item description
    file = "../data/IN3067-INM713_coursework_data_pizza_500.csv"
    output_file_name = "IN3067-INM713_coursework_data_pizza_500"

    solution = PartTwoSolution(file)

    task = "Subtask_RDF1"
    #task = "Subtask_RDF3"

    #Create RDF triples
    if task == "Subtask_RDF1":
        solution.performTask1()  #Fresh entity URIs
    elif task == "Subtask_RDF3_GoogleKG":
        solution.performTask2()  #Reusing URIs from Google KG
    # elif task == "Subtask_RDF3_WikidataKG":
    #     solution.performTask2()  #Reusing URIs from Wikidata KG

    #Graph with only data
    solution.saveGraph("./data/"+output_file_name+"-"+task+".ttl")